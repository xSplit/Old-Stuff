<?php

final class Vine
{
    public static function get($video)
    {
      return new VVideo(self::parse(file_get_contents($video)));
    }

    private static function parse($data)
    {
      $itemprops = explode(': {"liked":',explode('};',explode('window.POST_DATA = {',$data)[1])[0]);
      return json_decode('{"liked":'.$itemprops[1],true);
    }
}

class VVideo
{
    private $data;

    public function __construct(array $data)
    {
      $this->data = $data;
    }

    public function download($save=false)
    {
      $buff = file_get_contents($this->data['videoUrl']);
      if($save) file_put_contents($save,$buff);
      return $buff;
    }

    public function embed($p=true,$width=400,$height=400)
    {
      $embed = "<embed src='{$this->data['videoUrl']}' width='{$width}' height='{$height}'></embed>";
      if($p) echo $embed; else return $embed;
    }

    public function getData()
    {
      return $this->data;
    }

    public function getThumb()
    {
      return $this->data['thumbnailUrl'];
    }

    public function getDate()
    {
      return $this->data['created'];
    }

    public function getShareUrl()
    {
      return $this->data['shareUrl'];
    }

    public function getUser()
    {
      return $this->data['username'];
    }

    public function getUserId()
    {
      return $this->data['userIdStr'];
    }

    public function getDesc()
    {
      return $this->data['description'];
    }

    public function getTags()
    {
      return $this->data['tags'];
    }

    public function getVideoId()
    {
      return $this->data['shortId'];
    }

    public function getCountOf($type)
    {
      if(isset($this->data[$type]['count'])) //comments, reposts, likes
      return $this->data[$type]['count'];
      return false;
    }

    public function hasExplicitContents()
    {
      return $this->data['explicitContent'] > 0;
    }
} 
